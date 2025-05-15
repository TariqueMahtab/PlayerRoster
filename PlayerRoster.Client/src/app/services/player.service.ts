import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Player } from '../models/player';
import { environment } from '../../environments/environment'; 

@Injectable({
  providedIn: 'root'
})
export class PlayerService {
  private baseUrl = `${environment.apiUrl}/players`; 

  constructor(private http: HttpClient) { }

  getAll(): Observable<Player[]> {
    return this.http.get<Player[]>(this.baseUrl);
  }

  getById(id: number): Observable<Player> {
    return this.http.get<Player>(`${this.baseUrl}/${id}`);
  }

  create(player: Partial<Player>): Observable<Player> {
    return this.http.post<Player>(this.baseUrl, player);
  }

  update(id: number, player: Partial<Player>): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, player);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
