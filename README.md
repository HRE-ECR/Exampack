# Proforma Combiner (WPF, .NET 8)

Modern WPF rewrite of the original WinForms Proforma Combiner.

## What it does
- Loads AT200 or AT300 Excel list (configured in `appsettings.json`).
- Displays each row as a selectable tile.
- Exports selected items into a single combined PDF by importing requested pages from the source PDFs.
- Export shows a progress dialog with a live log of activity.

## Excel format
Required headers: `Title`, `Path`, `Page`.

`Page` supports: `3`, `1,3,5`, `2-6`, `4x3`, `2-4x2`.

## GitHub Actions
Builds on push to `main` and uploads the published output as an artifact.
