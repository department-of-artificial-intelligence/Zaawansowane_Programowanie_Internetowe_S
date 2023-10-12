class Blog {
    private _id:number;
    @required private _name:string;
    private _author:Author;
    private _articles: Article[];
    private static _objectCounter:number;


    constructor(id:number,name:string,author:Author,articles:Article[]){
        this._id = id;
        this._name = name;
        this._author = author;
        this._articles = articles;
        Blog._objectCounter++;
    }

    public addArticle(article:Article):void {
        this._articles.push(article);
    }

    public getArticlesTitles(articleTitles:Article):string[] {
        
    }

    public getArticle(title:Article):Article {
        return title;
    }
}

function required(message: string = "To pole jest wymagane")
: PropertyDecorator
{
return (target: object, name: string | symbol) => {
Object.defineProperty(target, name, {
get: function () { return this[`__${name.toString()}`]; },
set: function (value?: string) {
if (value === null || value === undefined || value.length
=== 0)
throw new Error(message);
this[`__${name.toString()}`] = value;
}
})
}
}