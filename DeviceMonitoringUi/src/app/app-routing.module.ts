import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';

import {DeviceDetailComponent} from './components/device-detail/device-detail.component';
import {DeviceListComponent} from './components/device-list/device-list.component';

const routes: Routes = [
  {path: '', component: DeviceListComponent},
  {path: 'device/:id', component: DeviceDetailComponent}
];

@NgModule({imports: [RouterModule.forRoot(routes)], exports: [RouterModule]})
export class AppRoutingModule {
}
