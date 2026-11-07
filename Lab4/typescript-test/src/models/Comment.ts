export class Comment {
    private _content: string;
    private _date: Date;
    private _replies: Comment[] = [];
    private _nick: string;

    constructor(content: string, nick: string) {
        this._content = content;
        this._nick = nick;
        this._date = new Date();
    }

    get content(): string {
        return this._content;
    }

    set content(value: string) {
        this._content = value;
    }

    get date(): Date {
        return this._date;
    }

    get nick(): string {
        return this._nick;
    }

    set nick(value: string) {
        this._nick = value;
    }

    addReply(reply: Comment): void {
        this._replies.push(reply);
    }

    getReplies(): Comment[] {
        return this._replies;
    }
}