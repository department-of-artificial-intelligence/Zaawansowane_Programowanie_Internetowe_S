import { Routes } from '@angular/router';
import { PagesListComponent } from './pages/pages-list.component';
import { AddPageComponent } from './pages/add-page.component';
import { PageDetailsComponent } from './pages/page-details.component';
import { PageRemoveComponent } from './pages/page-remove.component';

export const routes: Routes = [
  { path: '', component: PagesListComponent },
  { path: 'add', component: AddPageComponent },

  { path: 'pages', component: PagesListComponent },
  { path: 'pages/:id/details', component: PageDetailsComponent },
  { path: 'pages/:id/remove', component: PageRemoveComponent }
];

