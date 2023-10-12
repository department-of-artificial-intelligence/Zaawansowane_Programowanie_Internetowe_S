class Author {

    

    private _id:number;
    @required private _firstname:string;
    private _lastname:string;
    private _email:string;
    private static _objectCounter:number = 0;

    constructor(id:number,firstname:string,lastname:string,email:string){
        this._id = id;
        this._firstname = firstname;
        this._lastname = lastname;
        this._email = email;
        Author._objectCounter++;
    }

    
     
}



function required(target: object, name: string) {
    Object.defineProperty(target, name, {
        get: function () { return this[`__${name}`]; },
        set: function (value?: string) {
        if (value === null || value === undefined || value.length === 0)
        throw new Error(`${name} nie może być puste`);
        this[`__${name}`] = value;
        }
        })
}
