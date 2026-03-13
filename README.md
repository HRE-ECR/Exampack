# Proforma Combiner (WPF, .NET 8)

A modernized WPF rewrite of the original WinForms **Proforma Combiner**.

## What it does
- Loads AT200 or AT300 Excel list (configured in `appsettings.json`).
- Displays each row as a selectable **card tile** (Title + Page text).
- Exports the selected items into a **single combined PDF** by importing the requested page(s) from each source PDF.
- Export shows a progress dialog with a live log of actions (open file, add page, skip, errors).

## Excel format
The workbook must contain headers:
- `Title`
- `Path`
- `Page`

`Page` supports:
- single page: `3`
- list: `1,3,5` (also space or `;` separated)
- range: `2-6`
- copies: `4x3` or `4*3` (cap 100)
- range with copies: `2-4x2`

## Configuration
Edit `src/ProformaCombiner.Wpf/appsettings.json`:
```json
{
  "AT200ExcelPath": "I:/ServiceDelivery/ECR/1. Proforma Viewer/Databases/AT200 Proforma list.xlsx",
  "AT300ExcelPath": "I:/ServiceDelivery/ECR/1. Proforma Viewer/Databases/AT300 Performa List.xlsx",
  "SheetName": ""
}
```

## Build locally
```bash
cd src/ProformaCombiner.Wpf

dotnet restore

dotnet publish -c Release -r win-x64 --self-contained true   -p:PublishSingleFile=true   -p:IncludeNativeLibrariesForSelfExtract=true
```

## GitHub Actions
A workflow builds on every push to `main` and uploads the published output as an artifact.
