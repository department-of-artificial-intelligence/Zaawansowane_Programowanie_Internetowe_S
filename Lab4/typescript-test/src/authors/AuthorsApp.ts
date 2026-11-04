import { AuthorService } from "./AuthorService";
import { AuthorsView } from "./AuthorsView";

export class AuthorsApp {
    private service: AuthorService;
    private view: AuthorsView;

    constructor(container: HTMLElement, dataUrl: string = "dane.json") {
        this.service = new AuthorService(dataUrl);
        this.view = new AuthorsView(container);
    }

    async start(): Promise<void> {
        try {
            const authors = await this.service.getAuthors();
            console.log("Pobrani autorzy:", authors);
            this.view.render(authors);
        } catch (e) {
            this.view.showError((e as Error).message);
        }
    }
}
