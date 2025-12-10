import { Component, OnInit, Input, Output, EventEmitter }
from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { map } from 'rxjs';
import { Contact } from '../../Model/Contact';
import { ContactService } from '../../services/contacts-service.service';
@Component({
selector: 'app-contact',
templateUrl: 'contact.component.html',
styles: []
})
export class ContactComponent implements OnInit {
contact!: Contact;
constructor(private router: Router,
private activatedRoute: ActivatedRoute,
private contactService: ContactService) {
}
ngOnInit(): void {
this.activatedRoute.paramMap
.pipe(map((p: ParamMap) =>
parseInt(p.get('id') as string)
),
map((id: number) => this.contactService.findById(id)),
map((a: Contact | undefined) => {
if (a === undefined) {
throw new Error();
} else {
return a;
}
})
)
.subscribe(a => this.contact = a);
}
close() {
this.router.navigateByUrl("contacts");
}
}