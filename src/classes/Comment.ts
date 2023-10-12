class Comment {
    private _content:string;
    private _date:Date;
    private _replies:Comments[];
    private _nick:string;


    constructor(content:string,date:Date,replies:Comments[],nick:string){
        this._content = content;
        this._date = date;
        this._replies = replies;
        this._nick = nick;
    }

    public addReply():void {

    }

    public getReplies(): Comment[] {

    }
}