import { AsyncPipe, CommonModule, } from "@angular/common";
import { NgModule } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { RouterLink, RouterOutlet } from "@angular/router";
import { MatInputModule } from "@angular/material/input";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatIconModule } from "@angular/material/icon";
import { ReactiveFormsModule, FormsModule } from "@angular/forms";
import { MatMenuModule } from "@angular/material/menu";
import { HttpClientModule } from "@angular/common/http";
import { MdbDropdownModule } from 'mdb-angular-ui-kit/dropdown';
import { MdbRippleModule } from 'mdb-angular-ui-kit/ripple';

@NgModule({
  imports: [AsyncPipe,
    CommonModule,
    RouterOutlet,
    MatButtonModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    RouterLink,
    MatInputModule,
    ReactiveFormsModule, MatMenuModule, FormsModule, HttpClientModule,
    MdbDropdownModule, MdbRippleModule


  ],
  exports: [
    RouterOutlet,
    MatButtonModule,
    MatToolbarModule, MatButtonModule, MatIconModule, RouterLink
    , MatInputModule, ReactiveFormsModule, MatMenuModule, AsyncPipe,
    FormsModule, HttpClientModule, CommonModule, MdbDropdownModule, MdbRippleModule
  ]
})
export class AppMaterialModule { }
