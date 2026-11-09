export class Comment {
  private _text: string;
  private _date: Date;
  private _replies: Comment[];
  private _nickname: string;

  constructor(text: string, nickname: string) {
    this._text = text;
    this._date = new Date();
    this._replies = [];
    this._nickname = nickname;
  }

  get text(): string {
    return this._text;
  }

  set text(value: string) {
    this._text = value;
  }

  get date(): Date {
    return this._date;
  }

  set date(value: Date) {
    this._date = value;
  }

  get nickname(): string {
    return this._nickname;
  }

  set nickname(value: string) {
    this._nickname = value;
  }

  addReply(reply: Comment): void {
    this._replies.push(reply);
  }

  getReplies(): Comment[] {
    return this._replies;
  }
}