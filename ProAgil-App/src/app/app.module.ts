// modulos
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { AppRoutingModule } from './app-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// serviços
import { EventoService } from './_services/evento.service';

// componentes
import { AppComponent } from './app.component';
import { EventosComponent } from './eventos/eventos.component';
import { NavComponent } from './nav/nav.component';

// pipes
import { DateTimeFormatPipePipe } from './_helps/DateTimeFormatPipe.pipe';


@NgModule({
  declarations: [
    AppComponent,
      EventosComponent,
      NavComponent,
      DateTimeFormatPipePipe
   ],
  imports: [
    BrowserModule,
    BsDropdownModule.forRoot(), // para utilizar em toda a estrutura da app
    BsDatepickerModule.forRoot(),
    TooltipModule.forRoot(), // para utilizar em toda a estrutura da app
    ModalModule.forRoot(), // para utilizar em toda a estrutura da app
    BrowserAnimationsModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    EventoService
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule { }
