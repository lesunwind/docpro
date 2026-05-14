# PDF Feature Implementation Plan (Pre-Coding)

## Goal

Implement PDF support that mirrors the current DOCX feature set:

1. Parse input document into structured JSON hierarchy (`Section`, `TableInfo`, `ImageInfo`).
2. Support section path selection for extraction.
3. Generate output DOCX from extracted section(s), including text, tables, and images.
4. Expose the same behavior through Desktop UI and API endpoints.

This is a planning document only; no runtime PDF parsing code is included here.

---

## 1) Scope Alignment with Existing Word Functionality

The current Word pipeline behavior to match:

- Heading-based hierarchy with nested subsections.
- Section-level content aggregation.
- Table extraction with numbering/title heuristics.
- Image extraction and metadata capture (dimensions, content type, payload).
- JSON serialization to existing schema.
- Section extraction back into DOCX including recursive subsection rendering.

For PDF, there is no native "heading style" metadata like Word, so equivalent behavior will be achieved via layout heuristics and confidence scoring.

---

## 2) Target Architecture (for PDF)

Integrate with already-added abstractions:

- `IDocumentParser`
- `DocumentProcessingService`
- `DocumentTypeDetector`
- `DocumentParseResult`

### Planned parser

- Replace scaffold behavior in `PdfDocumentParser` with real implementation.
- Parser output will populate the same core models used by Word.

### Internal components to add

- `PdfTextExtractor` (page text blocks with coordinates)
- `PdfHeadingDetector` (section boundary inference)
- `PdfTableExtractor` (table cell reconstruction)
- `PdfImageExtractor` (embedded image extraction)
- `PdfSectionAssembler` (build hierarchy + content)
- `PdfParsingDiagnostics` (issues/warnings/confidence)

---

## 3) Functional Requirements

## FR-1: Input handling

- Accept `.pdf` from UI and API.
- Detect scanned/image-only PDFs and return explicit diagnostics.

## FR-2: Section hierarchy

- Infer headings from font size/weight/casing/position/numbering patterns.
- Create `Section` + nested `Subsections` tree.
- Record confidence for inferred headings in `Metadata`.

## FR-3: Section content

- Aggregate paragraph text under inferred section boundaries.
- Preserve reading order as reliably as possible.

## FR-4: Tables

- Detect tabular regions.
- Reconstruct rows/cells into `TableInfo.Data`.
- Infer table numbers/titles/captions via nearby text similar to Word handler behavior.

## FR-5: Images

- Extract embedded images and map to nearby section.
- Populate `ImageInfo` including content type, width/height, base64 payload.

## FR-6: JSON output

- Output must remain backward-compatible with current consumers.
- Include `ProcessingIssue` entries for uncertainty/failures.

## FR-7: Section extraction to DOCX

- Existing `SectionExtractor` should work unchanged on PDF-produced JSON.
- Validate generated DOCX parity against Word path behavior.

---

## 4) Non-Functional Requirements

- Performance: process medium manuals (<300 pages) within acceptable latency.
- Reliability: graceful degradation with warnings, not crashes.
- Determinism: same input should generally produce stable output.
- Observability: structured logs and issue metadata for troubleshooting.

---

## 5) Implementation Phases

## Phase 0 — Tooling Decision (1 review cycle)

Choose PDF library stack for .NET:

- Text/layout extraction library
- Table extraction strategy (library-assisted + heuristic fallback)
- Image extraction support
- OCR integration path for scanned PDFs (optional in initial phase)

**Deliverable:** approved dependency decision doc.

## Phase 1 — Baseline PDF Parser (MVP)

- Implement `PdfDocumentParser` end-to-end with:
  - text extraction
  - basic heading inference
  - section tree build
  - JSON output
- No table/image extraction required in first milestone; return diagnostics when skipped.

**Exit criteria:** PDF -> sections/content JSON works on representative samples.

## Phase 2 — Table Extraction

- Add table region detection and cell reconstruction.
- Reuse table numbering/caption heuristics compatible with `TableInfo`.
- Add duplicate/reference handling equivalent to Word where possible.

**Exit criteria:** tables present in JSON with acceptable fidelity.

## Phase 3 — Image Extraction

- Extract embedded images, dimensions, and content type.
- Map images to nearest section boundary.
- Populate `ImageInfo.Base64Data` for extractor compatibility.

**Exit criteria:** images represented and extractable into output DOCX.

## Phase 4 — Desktop Integration

- Ensure UI flow accepts PDF selection.
- Reuse existing parse/extract UX with no new required user steps.
- Display parser warnings in UI for low-confidence operations.

**Exit criteria:** UI parses and extracts from PDF-derived JSON.

## Phase 5 — API Hardening

- Validate `POST /api/v1/parse` for PDFs.
- Add clear API response diagnostics and status behavior.
- Add size/type validation and error contracts.

**Exit criteria:** API stable for DOCX + PDF in same endpoint.

## Phase 6 — Quality & Performance

- Add regression fixtures for PDFs.
- Add benchmark documents and timing thresholds.
- Tune heuristics and memory usage.

**Exit criteria:** CI-grade confidence and documented limits.

---

## 6) Data Model Extensions (Proposed)

Backward-compatible additions only:

- `Section.Metadata["heading_confidence"]`
- `Section.Metadata["source_page_start"]`, `source_page_end`
- `ProcessingIssue` entries for:
  - heading inference fallback
  - table extraction failure
  - image extraction failure
  - scanned PDF/OCR required

No schema-breaking changes are planned.

---

## 7) Testing Strategy

## Unit tests

- Heading inference heuristics.
- Section assembly with edge cases (multi-column, repeated headers/footers).
- Table parsing and caption association.
- Image extraction and dimension checks.

## Integration tests

- PDF fixture -> expected JSON snapshot.
- JSON -> DOCX extraction validation (content/tables/images presence).

## API tests

- Multipart PDF upload success.
- Empty/invalid PDF handling.
- Large file and timeout behavior.

---

## 8) Risks and Mitigations

- **Risk:** PDF reading order errors in complex layouts.
  - **Mitigation:** page-region sorting heuristics + issue flags.

- **Risk:** table extraction quality variance.
  - **Mitigation:** hybrid method (library + heuristics), confidence metadata.

- **Risk:** scanned PDFs lack text layer.
  - **Mitigation:** explicit diagnostic now; OCR integration as separate milestone.

- **Risk:** payload bloat from image base64.
  - **Mitigation:** optional future mode to externalize binary assets.

---

## 9) Review/Approval Checklist

Please review and approve before coding:

1. Proposed phase order
2. Library/tooling constraints (open-source/commercial allowed?)
3. Minimum acceptable fidelity for headings/tables/images
4. Whether OCR must be included in first release
5. Performance target expectations
6. Whether model extensions in `Metadata` are acceptable

Once approved, implementation will begin with Phase 0/1 and submit incremental PRs per phase.
