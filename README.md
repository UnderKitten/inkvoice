# Inkvoice

A small invoicing app I built for my own freelance work. It lets me enter my
business details, keep a list of clients, create invoices and estimates, and
export them as clean PDFs. Ontario HST is built in.

## What it does

- Store my business info (name, address, GST/HST number)
- Keep a list of clients
- Create invoices and estimates with line items (item, description, hours, rate)
- Work out the subtotal, HST, and total automatically
- Add comments or special instructions to a document
- Generate a PDF for any invoice or estimate
- A home dashboard with outstanding money, what has been paid this year, and recent documents

## Running it

The app runs locally in the browser. Nothing is online and no data leaves the computer.

To start it, double-click `run.bat`. A small window opens for the server and the
app opens in your browser at http://localhost:5099. If the page does not load
right away, give it a few seconds and refresh.

To stop it, close the server window.

If you prefer the command line, run this from the project folder:

```
dotnet run
```

## Where the data lives

Everything is kept in a single SQLite file called `invoices.db` in the project
folder. To back up, copy that file somewhere safe. It is not committed to git.

## Built with

- .NET 10, Blazor Server
- Entity Framework Core with SQLite
- QuestPDF for the PDFs

## Notes

- HST is fixed at 13 percent for Ontario.
- Invoice numbers are typed in by hand so I can use my own format.

## Things I might add later

- One click to turn an estimate into an invoice
- Suggest the next invoice number automatically
- Mark invoices paid and highlight overdue ones
- Search and filter the document list
