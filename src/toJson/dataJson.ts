import { Author } from "../classes/Author";
import { IShow } from "../infterfaces/IShow";


export class DataJson implements IShow{

    private _authors: Author[];

    constructor(authors: Author[]){
        this._authors = authors;
    }

    show(): string {
        return JSON.stringify(this._authors);
    }
    
}