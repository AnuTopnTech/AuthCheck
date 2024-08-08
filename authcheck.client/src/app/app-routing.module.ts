import { Component, NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './pages/login.component';
import { HomeComponent } from './pages/home/home.component';
import { RegisterComponent } from './pages/register/register.component';
import { AccountComponent } from './pages/account/account.component';
import { UsersComponent } from './pages/users/users.component';
import { roleGuard } from './guards/role.guard';
import { ForgetPasswordComponent } from './pages/forget-password/forget-password.component';
import { ResetPasswordComponent } from './pages/reset-password/reset-password.component';
import { ChangePasswordComponent } from './pages/change-password/change-password.component';
import { authGuard } from './guards/auth.guard';
import { CityComponent } from './pages/city/city.component';
import { AllLocationsComponent } from './all-locations/all-locations.component';

const routes: Routes = [
  {
    path: 'login', component: LoginComponent
  },
  {
    path: '', component: HomeComponent
  },
  {
    path: 'register', component: RegisterComponent
  },
  {
    path: 'account/:id', component: AccountComponent, canActivate: [authGuard]
  },
  {
    path: 'forget-password', component: ForgetPasswordComponent,
  },
  {
    path: 'reset-password', component: ResetPasswordComponent,
  },
  {
    path: 'change-password', component: ChangePasswordComponent,
  },

  {
    path: 'users', component: UsersComponent, canActivate: [roleGuard],
    data: {roles:['']}
  },
  { path: 'city', component: CityComponent, },
  { path: 'location', component: AllLocationsComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
  
})
export class AppRoutingModule { }
