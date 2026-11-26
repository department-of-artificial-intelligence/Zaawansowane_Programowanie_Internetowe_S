import { Article } from "./classes/article";
import { Author } from "./classes/author";
import { Blog } from "./classes/blog";
import { AuthorController } from "./controllers/AuthorController";
import { Counter } from "./counter";
import { measureTime } from "./decorators/measureTime";
import { HtmlListExporter } from "./exporters/HtmlListExporter";
import { HtmlNumberedListExporter } from "./exporters/HtmlNumberedListExporter";
import { HtmlTableExporter } from "./exporters/HtmlTableExporter";
import { JsonExporter } from "./exporters/JsonExporter";
import { AuthorService } from "./services/AuthorService";
import { ConsolePrintVisitor, Tree, TreeNode } from "./structures/Tree";
import "./style.css";
import type {
  ArticlePreview,
  AuthorDto,
  AuthorSignature,
  BlogHeader,
} from "./types/DomainTypes";

// const authors = [
//   new Author("Jan", "Kowalski", "jan@kowalski.pl"),
//   new Author("Anna", "Nowak", "anna@nowak.pl"),
// ];

// const controller = new AuthorController(new HtmlTableExporter());
// const tableHtml = controller.getAuthorsOutput(authors);

// controller.setExporter(new HtmlListExporter());
// const listHtml = controller.getAuthorsOutput(authors);

// controller.setExporter(new JsonExporter());
// const jsonOutput = controller.getAuthorsOutput(authors);

// const app = document.getElementById("app");
// if (app) {
//   app.innerHTML = `
//     <h2>Tabela HTML</h2>
//     ${tableHtml}

//     <h2>Lista HTML</h2>
//     ${listHtml}

//     <h2>JSON</h2>
//     <pre>${jsonOutput}</pre>
//   `;
// }
// let counter = new Counter("app");

//Zadania 5-6-7
// class App {
//   private _authorService: AuthorService;
//   private _authorController: AuthorController;
//   private _containerId: string;
//   private _outputContainer!: HTMLDivElement;

//   constructor(dataUrl: string, containerId: string) {
//     this._authorService = new AuthorService(dataUrl);
//     this._authorController = new AuthorController(new HtmlListExporter());
//     this._containerId = containerId;

//     this.initUI();
//     this.start();
//   }

//   private initUI(): void {
//     const container = document.getElementById(this._containerId);
//     if (container === null) return;

//     container.innerHTML = "";

//     const heading = document.createElement("h2");
//     heading.textContent = "Lista autorów";

//     const label = document.createElement("label");
//     label.textContent = "Wybierz formę wyświetlania: ";
//     const select = document.createElement("select");

//     const options = [
//       { value: "ul", text: "Lista punktowana" },
//       { value: "ol", text: "Lista numerowana" },
//       { value: "table", text: "Tabela" },
//     ];

//     options.forEach((opt) => {
//       const optionElement = document.createElement("option");
//       optionElement.value = opt.value;
//       optionElement.textContent = opt.text;
//       select.appendChild(optionElement);
//     });
//     select.addEventListener("change", () => {
//       this.changeFormat(select.value);
//     });

//     this._outputContainer = document.createElement("div");
//     this._outputContainer.style.marginTop = "15px";

//     container.appendChild(heading);
//     container.appendChild(label);
//     container.appendChild(select);
//     container.appendChild(this._outputContainer);
//   }

//   private start(): void {
//     this._authorService
//       .fetchAuthors()
//       .then(() => {
//         this.renderAuthors();
//       })
//       .catch((error) => {
//         console.error("Wystąpił błąd podczas pobierania autorów:", error);
//       });
//   }

//   private changeFormat(format: string): void {
//     switch (format) {
//       case "ul":
//         this._authorController.setExporter(new HtmlListExporter());
//         break;
//       case "ol":
//         this._authorController.setExporter(new HtmlNumberedListExporter());
//         break;
//       case "table":
//         this._authorController.setExporter(new HtmlTableExporter());
//         break;
//     }
//     this.renderAuthors();
//   }

//   private renderAuthors(): void {
//     const htmlContent = this._authorController.getAuthorsOutput(
//       this._authorService.authors
//     );
//     this._outputContainer.innerHTML = htmlContent;
//   }
// }

// new App("dane.json", "app");

//Zadanie 8

// const categoryTree = new Tree<string>("Programowanie");
// const webNode = new TreeNode<string>("Web Development");
// const tsNode = new TreeNode<string>("TypeScript");
// const jsNode = new TreeNode<string>("JavaScript");
// const mobileNode = new TreeNode<string>("Mobile");

// webNode.addChild(tsNode);
// webNode.addChild(jsNode);
// categoryTree.root.addChild(webNode);
// categoryTree.root.addChild(mobileNode);

// console.log("Odwiedzanie za pomocą wzorca Visitor");
// categoryTree.accept(new ConsolePrintVisitor<string>());
// const headAuthor = new Author("Jan", "Kowalski", "jan@kowalski.pl");
// const subAuthor1 = new Author("Anna", "Nowak", "anna@nowak.pl");
// const subAuthor2 = new Author("Piotr", "Kowalski", "piotr@kowalski.pl");

// const teamTree = new Tree<Author>(headAuthor);
// teamTree.root.addChild(new TreeNode<Author>(subAuthor1));
// teamTree.root.addChild(new TreeNode<Author>(subAuthor2));

// console.log("Odwiedzanie za pomocą funkcji strzałkowej");
// teamTree.traverse((author) => {
//   console.log(
//     `Autor: ${author.firstName} ${author.lastName} (${author.email})`
//   );
// });

//Zadanie 9
// const newAuthorData: AuthorDto = {
//   firstName: "Jan",
//   lastName: "Kowalski",
//   email: "jan@kowalski.pl",
// };

// const author = new Author(
//   newAuthorData.firstName,
//   newAuthorData.lastName,
//   newAuthorData.email
// );

// function formatSignature(signature: AuthorSignature): string {
//   return `Autor: ${signature.firstName} ${signature.lastName}`;
// }

// console.log(formatSignature(author));

// const article = new Article("Wprowadzenie do TypeScript", "Treść artykułu...");
// const blog = new Blog("Tech Blog", author);

// const preview: ArticlePreview = {
//   id: article.id,
//   title: article.title,
// };

// const header: BlogHeader = {
//   id: blog.id,
//   name: blog.name,
//   author: blog.author,
// };

// console.log("Podgląd artykułu:", preview);
// console.log("Nagłówek bloga:", header);

//Zadanie 10
// const authorService = new AuthorService("dane.json");

// const author1 = new Author("Jan", "Kowalski", "jan@kowalski.pl");
// const author2 = new Author("Anna", "Nowak", "anna@nowak.pl");

// authorService.addAuthor(author1);
// authorService.addAuthor(author2);

// console.log(
//   "Przed aktualizacją:",
//   author1.firstName,
//   author1.lastName,
//   author1.email
// );

// authorService.updateAuthor(author1.id, {
//   email: "jan.kowalski@nowadomena.pl",
// });

// console.log(
//   "Po zmianie samego emaila:",
//   author1.firstName,
//   author1.lastName,
//   author1.email
// );

// authorService.updateAuthor(author1.id, {
//   firstName: "Janusz",
//   lastName: "Kowalski-Nowak",
// });

// console.log(
//   "Po zmianie imienia i nazwiska:",
//   author1.firstName,
//   author1.lastName,
//   author1.email
// );

// try {
//   authorService.updateAuthor(author1.id, { email: "bledny-adres" });
// } catch (error) {
//   console.error(
//     "Walidacja zadziałała przy częściowej aktualizacji:",
//     (error as Error).message
//   );
// }
//Zadanie 11-12
// class App {
//   private _authorService: AuthorService;

//   constructor(dataUrl: string) {
//     this._authorService = new AuthorService(dataUrl);
//     this.start();
//   }

//   private start(): void {
//     this._authorService
//       .fetchAuthors()
//       .then((authorsRecord) => {
//         console.log("Pobrani autorzy jako Record:", authorsRecord);
//         const author = this._authorService.getAuthorById(3);
//         if (author) {
//           console.log(
//             `Znaleziono autora o ID 3: ${author.firstName} ${author.lastName}`
//           );
//         }
//         const readonlyAuthors = this._authorService.getReadonlyAuthors();
//         this.renderAuthorsList(readonlyAuthors);
//         Object.values(authorsRecord).forEach((a) => {
//           console.log(`[${a.id}] ${a.firstName} ${a.lastName} (${a.email})`);
//         });
//       })
//       .catch((error) => {
//         console.error("Wystąpił błąd:", error);
//       });
//   }
//   private renderAuthorsList(authors: readonly Readonly<Author>[]): void {
//     const container = document.getElementById("app");
//     if (container === null) return;

//     const ul = document.createElement("ul");

//     authors.forEach((author: Readonly<Author>) => {
//       const li = document.createElement("li");
//       li.textContent = `${author.firstName} ${author.lastName} (${author.email})`;
//       ul.appendChild(li);

//       //author.firstName = "Haker";
//     });

//     //authors.push(new Author("Nowy", "Autor", "nowy@autor.pl"));

//     container.innerHTML = "<h2>Lista autorów (tylko do odczytu)</h2>";
//     container.appendChild(ul);
//   }
// }

// new App("dane.json");
//Zadanie 13
class App {
  private _authorService: AuthorService;

  constructor(dataUrl: string) {
    this._authorService = new AuthorService(dataUrl);
    this.start();
  }

  private start(): void {
    this._authorService
      .fetchAuthors()
      .then(() => {
        this.displayAuthorSearch(1);
        this.displayAuthorSearch(999);
      })
      .catch((error) => {
        console.error("Wystąpił błąd:", error);
      });
  }

  private displayAuthorSearch(id: number): void {
    const author = this._authorService.getAuthorById(id);

    const resultMessage =
      (author?.firstName && `${author.firstName} ${author.lastName}`) ??
      "Nie znaleziono autora";

    console.log(`Wynik wyszukiwania (ID: ${id}): ${resultMessage}`);

    const container = document.getElementById("app");
    if (container !== null) {
      const p = document.createElement("p");
      p.textContent = `ID ${id}: ${resultMessage}`;
      container.appendChild(p);
    }
  }
}

new App("dane.json");
const testAuthors = [
  new Author("Jan", "Kowalski", "jan@kowalski.pl"),
  new Author("Anna", "Nowak", "anna@nowak.pl"),
];
const controller = new AuthorController(new HtmlTableExporter());
console.log(controller.getAuthorsOutput(testAuthors));
