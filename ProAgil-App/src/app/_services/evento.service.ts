import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Evento } from '../_models/Evento';

// providedIn: 'root' => para intejar em qualquer componente
@Injectable({
  providedIn: 'root'
})
export class EventoService {
  baseURL = 'http://localhost:5000/api/evento';

constructor(private http: HttpClient) { }

// todos os eventos
getAllEvento(): Observable<Evento[]>{
  return this.http.get<Evento[]>(this.baseURL);
}

// eventos por tema
getEventoByTema(tema: string): Observable<Evento[]>{
  return this.http.get<Evento[]>('${this.baseURL}/getByTema/${tema}'); // criar URL
}

// eventos por Id
getEventoById(id: number): Observable<Evento[]>{
  return this.http.get<Evento[]>('${this.baseURL}/${id}'); // criar URL
}

// criar eventos
postEvento(evento: Evento){
  return this.http.post(this.baseURL, evento); // criar URL , o evento ve do eventos.ts -> metodo salvarAlteracao
}

// update eventos
putEvento(evento: Evento) {
  return this.http.put(`${this.baseURL}/${evento.id}`, evento); // criar URL , o evento ve do eventos.ts -> metodo salvarAlteracao
}

// apagar eventos
deleteEvento(id: number) {
  return this.http.delete(`${this.baseURL}/${id}`);
}
}