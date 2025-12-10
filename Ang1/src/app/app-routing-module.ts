import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ContactsListComponent } from '.Components/contacts-list/contacts-list.component';
import { ContactComponent } from './Components/contact/contact.component.ts';
import { CreateContactComponent } from './Components/create-contact/create-contact.component';

const routes: Routes = [
{ path: "", component: ContactsListComponent },
{ path: "create", redirectTo:"contacts/create"},
{ path: "contacts", component: ContactsListComponent, children: [
{ path: "create", component: CreateContactComponent },
{ path: ’:id’, component: ContactComponent },
]},
{ path: "**", component: ContactsListComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }
