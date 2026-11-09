
class Comments {
    private _content: string;
    private _date: Date;
    private _answers: Comments[];
    private _nickname: string;

    constructor(content: string, nickname: string){
        this._content = content;
        this._nickname = nickname;
    }

    get content():string { return this._content}
    set content(value:string) { this._content = value; }
    get date():Date { return this._date}
    set date(value:Date) { this._date= value; }
    get nickname():string { return this._nickname}
    set nickname(value:string) { this._nickname= value; }

    addAnswer(answer: Comments) {
        this._answers.push(answer);
    }
    getAnswer(){
        return this._answers;
    }
}