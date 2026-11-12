import { Autor } from "../domain/Autor";

export class AutorListComponent {
    private container: HTMLElement;
    private selectElement: HTMLSelectElement;
    private contentContainer: HTMLElement;
    private autorzy: ReadonlyArray<Readonly<Autor>> = [];

    constructor() {
        this.container = document.createElement("div");

        this.selectElement = document.createElement("select");
        this.selectElement.innerHTML = `
      <option value="ul">Lista punktowana</option>
      <option value="ol">Lista numerowana</option>
      <option value="table">Tabela</option>
    `;

        this.selectElement.addEventListener("change", () => this.renderContent());

        this.contentContainer = document.createElement("div");
        this.contentContainer.style.marginTop = "15px";

        this.container.appendChild(this.selectElement);
        this.container.appendChild(this.contentContainer);
    }

    public mount(target: HTMLElement): void {
        target.appendChild(this.container);
    }

    public render(autorzy: ReadonlyArray<Readonly<Autor>>): void {
        this.autorzy = autorzy;
        this.renderContent();
    }

    private renderContent(): void {
        this.contentContainer.innerHTML = "";
        if (this.autorzy.length === 0) return;

        const mode = this.selectElement.value;

        if (mode === "ul" || mode === "ol") {
            const list = document.createElement(mode);
            this.autorzy.forEach((a) => {
                const li = document.createElement("li");
                li.textContent = `${a.imie} ${a.nazwisko} (${a.email})`;
                list.appendChild(li);
            });
            this.contentContainer.appendChild(list);
        } else if (mode === "table") {
            const table = document.createElement("table");
            table.border = "1";
            table.style.borderCollapse = "collapse";
            table.style.padding = "5px";

            table.innerHTML = `
        <thead>
          <tr>
            <th style="padding: 5px;">ID</th>
            <th style="padding: 5px;">Imię</th>
            <th style="padding: 5px;">Nazwisko</th>
            <th style="padding: 5px;">Email</th>
          </tr>
        </thead>
        <tbody>
          ${this.autorzy
                    .map(
                        (a) => `<tr>
                <td style="padding: 5px;">${a.id}</td>
                <td style="padding: 5px;">${a.imie}</td>
                <td style="padding: 5px;">${a.nazwisko}</td>
                <td style="padding: 5px;">${a.email}</td>
              </tr>`
                    )
                    .join("")}
        </tbody>
      `;
            this.contentContainer.appendChild(table);
        }
    }
}