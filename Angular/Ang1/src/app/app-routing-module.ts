import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ContactsListComponent } from '.Components/contacts-list/contacts-list.component.html';
import { CreateContactComponent } from './Components/create-contact/create-contact.component';

const routes: Routes = [
  { path: '', component: ContactsListComponent, pathMatch: 'full' },
  {
    path: 'contacts',
    component: ContactsListComponent,
    children: [{ path: 'create, component: CreateContactComponent' }],
  },
  { path: 'create', component: CreateContactComponent },
  { path: '**', component: ContactsListComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
