import { Author } from "../models/Author";
import { AuthorExporter } from "../formatters/AuthorExporter";
import { FormatterFactory, type FormatType } from "../formatters/FormatterFactory";

export class AuthorsView {
    private static readonly OPTIONS: { value: FormatType, label: string }[] = [
        { value: "ul", label: "Lista punktowana" },
        { value: "ol", label: "Lista numerowana" },
        { value: "table", label: "Tabela" }
    ];

    private exporter = new AuthorExporter(FormatterFactory.create("ul"));
    private authors: Author[] = [];
    private select: HTMLSelectElement;
    private output: HTMLDivElement;

    constructor(container: HTMLElement) {
        const label = document.createElement("label");
        label.textContent = "Sposób wyświetlania: ";

        this.select = document.createElement("select");
        for (const option of AuthorsView.OPTIONS) {
            this.select.add(new Option(option.label, option.value));
        }
        label.appendChild(this.select);

        this.output = document.createElement("div");
        container.append(label, this.output);

        this.select.addEventListener("change", () => {
            // swap the strategy at runtime
            this.exporter.setFormatter(FormatterFactory.create(this.select.value as FormatType));
            this.refresh();
        });
    }

    render(authors: Author[]): void {
        this.authors = authors;
        this.refresh();
    }

    showError(message: string): void {
        this.output.textContent = message;
    }

    private refresh(): void {
        // the formatters escape the data, so innerHTML is safe here
        this.output.innerHTML = this.exporter.export(this.authors);
    }
}
