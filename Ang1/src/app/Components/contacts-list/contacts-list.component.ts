‘import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Contact } from 'src/app/Model/contact';
import { ContactService } from 'src/app/Services/contact.service';
@Component({
selector: 'app-contacts-list',
templateUrl: `./contacts-list.component.html`,
styles: []
})
export class ContactsListComponent implements OnInit {
isListShown: boolean;
title: string;
contacts$: Observable<Contact[]>;
Sex = Sex;
constructor(contactsService: ContactService) {
this.isListShown = true;
this.title = "Ukryj listę"
this.contacts$ = contactsService.contacts$;
}
ngOnInit(): void {
}
onClick() {
this.isListShown = !this.isListShown;
this.title = this.isListShown ? "Ukryj listę" : "Pokaż listę";
}
}