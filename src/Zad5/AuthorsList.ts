import { Author } from "../classes/Author";

export class AuthorList {
    private _authors: Author[] = [];

    async fetchAuthorsFromServer() {
        try {
            const response = await fetch('../build/data.json');
            if(!response.ok){
                throw new Error("Błąd podczas pobierania danych");
            }

            const data = await response.json();

            this._authors = data.map((authorData:any) => {
                return new Author(
                    authorData.firstName,
                    authorData.lastName,
                    authorData.email
                );
            });

        }catch (error){
            console.error("Błąd podczas pobierania z pliku");
        }
    }

    showAuthors(){
        this.fetchAuthorsFromServer()
            .then(()=> {
                console.log(this._authors);
        });
    }
}

const authorList = new AuthorList();
authorList.showAuthors();