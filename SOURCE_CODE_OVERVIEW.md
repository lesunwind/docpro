# DocumentProcessor: Current Source-Code Behavior

This document describes what the current codebase does today.

## Solution layout

- `DocumentProcessor.Core`: shared parsing/extraction logic and data models.
- `DocumentProcessor.UI`: WinForms app to parse DOCX into JSON and extract selected sections.
- `DocumentProcessor.Console`: console harness with hard-coded extraction test flows.
- `DocumentProcessor.Tests`: MSTest test project for parser and extractor behavior.

## Core data model

The parser/extractor pipeline revolves around a hierarchical `Section` model:

- `Section` contains:
  - `Title`
  - `Level` (derived from heading style, e.g., Heading1 -> 1)
  - `Content` (paragraph text concatenated with newlines)
  - `Subsections` (nested `Section` objects)
  - `Tables` (`TableInfo` list)
  - `Images` (`ImageInfo` list)
  - `Metadata` dictionary

`TableInfo` captures table identity, inferred numbering/caption, cell data, duplicate flags, and cross-references. `ImageInfo` stores image type, dimensions, and base64 payload used for reconstruction.

## DOCX -> JSON parsing behavior (`WordDocumentParser`)

`WordDocumentParser` reads a `.docx` file with OpenXML and writes structured JSON to the configured output folder.

### Section detection

- Iterates top-level elements in document body.
- Treats paragraphs with style IDs starting with `Heading` as section boundaries.
- Heading number suffix determines level (`Heading2` => level 2).
- Maintains a stack to build parent/child subsection nesting.

### Section content accumulation

- Non-heading paragraph text is appended to current section content.
- Non-empty text lines are joined by newline and stored in `Section.Content` when a section closes.

### Table extraction

- Collects `Table` elements seen within a section’s element window.
- Uses nearby preceding paragraph text to infer table number/title via regex.
- If number already exists, marks table as duplicate and links `OriginalTableId`.
- Stores all rows/cells as `List<List<string>>` text.

### Image extraction

- Traverses section elements for `Drawing` nodes and extracts embedded image parts.
- Pulls content type, original dimensions, and binary bytes.
- Serializes bytes to base64 and stores them in `ImageInfo`.

### Output

- Serializes `List<Section>` using indented JSON.
- Nulls are ignored by serializer options.
- Output filename defaults to `document_structure.json`, but callers can override it.

## JSON -> DOCX extraction behavior (`SectionExtractor`)

`SectionExtractor` recreates a new Word document from parsed JSON.

### Target section lookup and extraction

- Accepts a `sectionPath` string array (e.g., `GENERAL INFORMATION > System Description > Configurations`).
- Finds the target section in JSON hierarchy.
- Creates output DOCX and applies document styles.

### Rendering behavior

Two extraction paths exist in code:

1. **Single target section render variant(s)** (`ExtractSectionToNewDocument_v0219`, backup variant):
   - Writes selected section title/content.
   - Appends section tables and images.

2. **Recursive hierarchy render** (`ExtractSectionToNewDocument`):
   - Processes selected section and all nested subsections recursively.
   - For each section: title, content, tables, then images.

### Image reinsertion

- Decodes stored base64 image data.
- Adds image part to main document part.
- Inserts inline drawing with scaled dimensions (`ScaleFactor = 0.65`).

## UI app behavior (`DocumentProcessor.UI`)

`MainForm` provides two functional tabs:

1. **Parsing tab**
   - Browse/select DOCX.
   - Copies selected file into app `input` folder.
   - Parses to JSON in app `output` folder (filename = input basename + `.json`).
   - Can open resulting JSON in Notepad.

2. **Extraction tab**
   - Lists JSON files from output folder.
   - Builds a tree view of section/subsection titles.
   - User checks one or more sections to extract.
   - Creates output DOCX derived from selected nodes.

At startup, app ensures `input`, `output`, and `logs` directories exist and initializes file-based logging.

## Console app behavior (`DocumentProcessor.Console`)

`Program.Main` currently runs hard-coded extraction test methods rather than a CLI parser:

- `TestExtraction_Configuration()`
- `TestExtraction_OperatingSites()`

Each test:

- Uses `input/document_structure.json`.
- Extracts a predefined section path.
- Writes output DOCX in `output/`.
- Prints basic success/failure and file-size diagnostics.

Other test helpers remain in source but are commented out or not called from `Main`.

## Tests

The test project includes parser and section extraction tests against sample fixtures (`TestFiles/TestDocument.docx`, `TestFiles/document_structure.json`).

## Practical implications of current implementation

- Section detection depends on Word heading styles; unstyled headings will not create hierarchy nodes.
- Table numbering is inferred from nearby text and may fall back to auto numbering (`Table N`).
- JSON can become large because embedded images are stored as base64 text.
- Console project is currently a developer harness (fixed scenarios), not a user-facing command-line tool.
