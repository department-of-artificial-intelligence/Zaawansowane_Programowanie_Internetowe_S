import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Category } from '../../Model/Category';
import { Sex, Contact } from '../../Model/Contact';
import { Email } from "../../Model/Email";
import { ContactService } from '../../services/contacts-service.service';
@Component({
selector: 'app-create-contact',
templateUrl: 'create-contact.component.html',
styles: [
]
})
export class CreateContactComponent {
contact: {
firstName: string;
lastName: string;
email: string;
sex: string | null;
age: number | null;
}
@ViewChild(NgForm) form!: NgForm;
isModalShown: string;
constructor(private contactService: ContactService,
private router: Router) {
this.contact = {
firstName: "",
lastName: "",
email: "",
sex: null,
age: null
}
this.isModalShown = "none";
}
showDialog() {
this.isModalShown = "block";
}
cancelClose() {
this.isModalShown = "none";
}
confirmClose() {
this.isModalShown = "none";
this.router.navigateByUrl("contacts");
}
close() {
if(this.form.dirty)
this.isModalShown = "block";
else
this.router.navigateByUrl("contacts");
}
submit(form:NgForm) {
if(form.valid)
{
this.contactService.addContact(
new Contact(1,
this.contact.firstName,
this.contact.lastName,
this.contact.sex == "Male" ? Sex.Male : Sex.
Female,
new Email(this.contact.email),
this.contact.age
)
);
this.router.navigateByUrl("contacts");
}
else
{
Object.keys(form.controls)
.forEach(key =>form.controls[key].markAsDirty());
}
}
}