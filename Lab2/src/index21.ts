import {ajax} from 'rxjs/ajax'
import { map } from 'rxjs/operators';

ajax("dane.json")
    .pipe(map(odp=>odp.response))
    .subscribe(e=>console.log(e));