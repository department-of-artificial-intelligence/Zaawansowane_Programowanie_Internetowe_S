

class Article {
    private _id:number;
    @required private _title:string;
    private _content:string;
    private _creationDate:Date;
    private _comments:Comment[];
    private static _objectCounter:number;


    constructor(id:number,title:string,content:string,creationDate:Date,comments:Comments[]){
        this._id = id;
        this._title = title;
        this._content = content;
        this._creationDate = creationDate;
        this._comments = comments;
    }

    public addComment(comment:Comment):void {
        this._comments.push(comment);
    }

    public getComments():Comment[] {
        return this._comments;
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


