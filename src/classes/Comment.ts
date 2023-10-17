
export class Comment {
    private _content:string;
    private _date:Date;
    private _replies:Comment[];
    private _nick:string;


    constructor(content:string,date:Date,replies:Comment[],nick:string){
        this._content = content;
        this._date = date;
        this._replies = replies;
        this._nick = nick;
    }

    public addReply(reply:Comment):void {
        this._replies.push(reply);
    }

    public getReplies(): Comment[] {
        return this._replies
    }
}