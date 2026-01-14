import { Component, OnInit } from '@angular/core';
//import { Contact } from './Model/app'

@Component({
  selector: 'app-contacts-list',
  standalone: false,
  templateUrl: './contacts-list.component.html',
  styleUrl: './contacts-list.component.css'
})
export class ContactsListComponent implements OnInit {
  //contacts: Contact[];

  constructor() {
    //this.contacts = [];
  }

  ngOnInit(): void {

  }
}
