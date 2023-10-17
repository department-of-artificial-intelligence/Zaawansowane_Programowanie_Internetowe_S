import {validate} from '../Validators'
import { Comment } from './Comment';

export class Article {
    private _id:number;

    @validate([{
        validator: (title:string) => {
            if(!title || title.trim() === ''){
                return false
            }
            return true
        },
        message: 'Title is empty'
    },
    ]) 
    private _title:string;
    private _content:string;
    private _creationDate:Date;
    private _comments:Comment[];
    private static _objectCounter:number;


    constructor(id:number,title:string,content:string,creationDate:Date,comments:Comment[]){
        this._id = id;
        this._title = title;
        this._content = content;
        this._creationDate = new Date();
        this._comments = comments;
    }

    get id():number {return this._id}
    get title():string {return this._title}

    public addComment(comment:Comment):void {
        this._comments.push(comment);
    }

    public getComments():Comment[] {
        return this._comments;
    }
}






