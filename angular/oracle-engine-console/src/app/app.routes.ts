import { Routes } from '@angular/router';
import { DesignSystemComponent } from './pages/design-system/design-system.component';
import { AppComponent } from './app.component';
import { IndexComponent } from './pages/index/index.component';

export const routes: Routes = [
    { path: 'design-system', component: DesignSystemComponent },
    { path: '', component: IndexComponent}
];
