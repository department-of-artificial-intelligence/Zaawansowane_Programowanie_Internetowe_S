import { Buttons } from "./Zad4/Buttons";
import { AuthorList } from "./Zad5/AuthorsList";

function initialize() {
    const app = new Buttons();
}

window.addEventListener('load',initialize);

const app = new AuthorList();
app.showAuthors();