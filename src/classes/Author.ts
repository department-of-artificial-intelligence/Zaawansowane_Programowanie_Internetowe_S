import { validateEmail,required } from "../Validators";

export class Author {

    

    private _id:number;
    @required
    private _firstname:string;
    @required
    private _lastname:string;
    @required
    private _email:string;
    private static _objectCounter:number = 0;

    constructor(firstname:string,lastname:string,email:string){
        this._id = Author._objectCounter++;
        this._firstname = firstname;
        this._lastname = lastname;
        if(!validateEmail(email)){
            throw new Error('That email is incorrect');
        }
        this._email = email;
    }

    get firstName():string {return this._firstname}
    get lastName():string {return this._lastname}
    get email():string {return this._email}
     
}



