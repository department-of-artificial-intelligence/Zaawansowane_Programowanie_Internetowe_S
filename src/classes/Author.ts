import { required,validateEmail } from "../Validators";

export class Author {

    

    private _id:number;
    @required 
    private _firstname:string;
    @required
    private _lastname:string;
    @required
    private _email:string;
    private static _objectCounter:number = 0;

    constructor(id:number,firstname:string,lastname:string,email:string){
        this._id = id;
        this._firstname = firstname;
        this._lastname = lastname;
        if(!validateEmail(email)){
            throw new Error('That email is incorrect');
        }
        this._email = email;
        Author._objectCounter++;
    }

    get firstName():string {return this._firstname}
    get lastName():string {return this._lastname}
    get email():string {return this._email}
     
}



