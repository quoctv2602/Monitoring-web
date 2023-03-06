import { RouterModule } from '@angular/router';
import { PagesLoginComponent } from './pages-login/pages-login.component';

export let COMPONENT_Pages = [PagesLoginComponent];
export let Router_Page = [
  RouterModule.forRoot([{ path: 'login', component: PagesLoginComponent }]),
];
