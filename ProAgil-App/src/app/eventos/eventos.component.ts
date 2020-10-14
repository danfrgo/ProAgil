import { Component, OnInit, TemplateRef } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Evento } from '../_models/Evento';
import { EventoService } from '../_services/evento.service';

@Component({
  selector: 'app-eventos',
  templateUrl: './eventos.component.html',
  styleUrls: ['./eventos.component.scss']
})
export class EventosComponent implements OnInit {

  eventosFiltrados: Evento[];
  eventos: Evento[];
  imagemLargura = 50; // em pixeis
  imagemMargem = 2;
  mostrarImagem = false;
  modalRef: BsModalRef;

  _filtroLista = '';

  constructor(private eventoService: EventoService
   , private modalService: BsModalService
  ) {}

  get filtroLista(): string {
    return this._filtroLista;
  }

  set filtroLista(value: string) {
    this._filtroLista = value;
    // os eventosFiltrados recebem ou o resultado da funçao this.filtrarEventos(this._filtroLista) ou recebe os proprios eventos..
    // ... que vieram da propria API "this.eventos"
    this.eventosFiltrados = this.filtroLista ? this.filtrarEventos(this._filtroLista) : this.eventos;
  }

openModal(template: TemplateRef<any>){
  this.modalRef = this.modalService.show(template);
  
}

  ngOnInit() {
    this.getEventos();
  }

  filtrarEventos(filtrarPor: string): Evento[] {
    filtrarPor = filtrarPor.toLocaleLowerCase();
    return this.eventos.filter(
      evento => evento.tema.toLocaleLowerCase().indexOf(filtrarPor) !== -1
    );
  }

  alternarImagem() {
    this.mostrarImagem = !this.mostrarImagem;
  }

  getEventos() {
    // Serviço http encapsolado dentro do getEvento
    this.eventoService.getAllEvento().subscribe(
      (_eventos: Evento[]) => {
        this.eventos = _eventos;
        this.eventosFiltrados = this.eventos;
        console.log(_eventos);
      }, error => {
        console.log(error);
      });
  }


}
