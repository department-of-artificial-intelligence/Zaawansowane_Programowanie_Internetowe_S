import { Component, OnInit } from '@angular/core';
import { Contact, Sex } from '../Model/contact';
import { Email } from '../Model/Email';

@Component({
  selector: 'app-contact-list',
  standalone: false,
  templateUrl: './contact-list.component.html',
  styleUrl: './contact-list.component.css',
})
export class ContactListComponent implements OnInit {
  contacts: Contact[];

  constructor() {
    this.contacts = [];
  }

  ngOnInit(): void {
    this.contacts.push(
      new Contact(
        1,
        'Ala',
        'Kot',
        Sex.Female,
        new Email('ala.kot@przyklad.pl'),
        23
      ),
      new Contact(
        2,
        'Tomasz',
        'Nowak',
        Sex.Male,
        new Email('tomasz.nowak@przyklad.pl'),
        34
      ),
      new Contact(
        3,
        'Cezary',
        'Adamski',
        Sex.Male,
        new Email('cezary.adamski@przyklad.pl'),
        45
      )
    );
  }
}
