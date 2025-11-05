import { Autor } from "./Autor";
import { AutorExporter } from "./AutorExporter";
import { HtmlTableFormatter } from "./HtmlTableFormatter";
import { JsonFormatter } from "./JsonFormatter";
import { Blog } from "./Blog"
import { Komentarz } from "./Komentarz"
import { Artykul } from "./Artykul"


const autor = new Autor("Jan", "Kowal", "jan.kowalski@gmail.com"); 

const blog = new Blog("Blog EZ", autor); 

const artykul = new Artykul("Nowy artykul", "Opis asdasd"); 

blog.dodajArtykul(artykul); 

const komentarz = new Komentarz("artykul", "user123"); 

artykul.dodajKomentarz(komentarz); 

console.log(artykul);
console.log(blog.pobierzTytulyArtykulow()); 
console.log(artykul.pobierzKomentarze());



const autorzy: Autor[] = [
    new Autor("Jan", "Kowal", "jan.kowal@example.com"),
    new Autor("Anna", "Nowak", "anna.nowak@example.com")
];


let exporter = new AutorExporter(new HtmlTableFormatter());
console.log("HTML TABLE");
console.log(exporter.export(autorzy));

exporter = new AutorExporter(new JsonFormatter());
console.log("JSON");
console.log(exporter.export(autorzy));



// Counter App
class CounterApp {
    private input: HTMLInputElement;
    private plusBtn: HTMLButtonElement;
    private minusBtn: HTMLButtonElement;

    constructor(inputId: string, plusBtnId: string, minusBtnId: string) {
        this.input = document.getElementById(inputId) as HTMLInputElement;
        this.plusBtn = document.getElementById(plusBtnId) as HTMLButtonElement;
        this.minusBtn = document.getElementById(minusBtnId) as HTMLButtonElement;

        this.initEvents();
    }

    private initEvents(): void {
        this.plusBtn.addEventListener("click", () => this.increment());
        this.minusBtn.addEventListener("click", () => this.decrement());
    }

    private increment(): void {
        this.input.value = (parseInt(this.input.value) + 1).toString();
    }

    private decrement(): void {
        this.input.value = (parseInt(this.input.value) - 1).toString();
    }
}

window.onload = () => {
    new CounterApp("counterInput", "plusBtn", "minusBtn");
};



