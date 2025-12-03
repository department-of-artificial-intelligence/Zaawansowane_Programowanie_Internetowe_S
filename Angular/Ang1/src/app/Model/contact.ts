import { Email } from './email';


export enum Sex {
    Male,
    Female
}

export class Contact {
    constructor(public id: number,
                public firstName:string,
                public lastName:string,
                public sex:Sex,
                public email:Email[],
                public age:number
    ) {}
}
