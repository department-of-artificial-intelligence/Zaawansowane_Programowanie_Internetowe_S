import { required, validate } from '../Validators';
import { Article } from './Article';
import { Author } from './Author'; 

export class Blog {
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
    private _name:string;
    @required
    private _author:Author;
    private _articles: Article[];
    private static _objectCounter:number;


    constructor(name:string,author:Author,articles:Article[]){
        this._id = Blog._objectCounter++;
        this._name = name;
        this._author = author;
        this._articles = articles;
    }

    public addArticle(article:Article):void {
        this._articles.push(article);
    }

    public getArticlesTitles():string[] {
        const titltesArray: string[] = [];
        for(const article of this._articles){
            titltesArray.push(article.title)
        }
        return titltesArray
    }

    public getArticle(index:number):Article | undefined{
        if(index >= 0 && index < this._articles.length){
            return this._articles[index];
        }
        else{
            return undefined;
        }
    }
}

