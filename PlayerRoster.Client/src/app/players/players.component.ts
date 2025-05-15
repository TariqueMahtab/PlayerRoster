import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PlayerService } from '../services/player.service';
import { Player } from '../models/player';
import { NavbarComponent } from '../navbar/navbar.component';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-players',
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent],
  templateUrl: './players.component.html',
  styleUrls: ['./players.component.scss']
})
export class PlayersComponent implements OnInit {
  players: Player[] = [];
  loading = true;
  error: string | null = null;

  sortField: keyof Player = 'fullName';
  sortAsc: boolean = true;

  form = {
    fullName: '',
    position: '',
    ppg: null as number | null,
    rpg: null as number | null,
    apg: null as number | null,
  };

  editingId: number | null = null;

  constructor(private playerService: PlayerService) { }

  ngOnInit(): void {
    this.loadPlayers();
  }

  loadPlayers(): void {
    this.loading = true;
    this.playerService.getAll().subscribe({
      next: (data: Player[]) => {
        this.players = data;
        this.applySort();
        this.loading = false;
      },
      error: (err: any) => {
        this.error = err.message;
        this.loading = false;
      }
    });
  }

  applySort(): void {
    const field = this.sortField;
    this.players.sort((a, b) => {
      const valA = a[field];
      const valB = b[field];
      if (typeof valA === 'string' && typeof valB === 'string') {
        return this.sortAsc ? valA.localeCompare(valB) : valB.localeCompare(valA);
      }
      return this.sortAsc
        ? (valA as number) - (valB as number)
        : (valB as number) - (valA as number);
    });
  }

  onSortOrderChange(event: Event): void {
    this.sortAsc = (event.target as HTMLSelectElement).value === 'true';
    this.applySort();
  }

  getSortArrow(field: keyof Player): string {
    return this.sortField === field ? (this.sortAsc ? '▲' : '▼') : '';
  }

  startEdit(player: Player): void {
    this.editingId = player.id;
    this.form = {
      fullName: player.fullName,
      position: player.position,
      ppg: player.ppg,
      rpg: player.rpg,
      apg: player.apg,
    };
  }

  cancelEdit(): void {
    this.editingId = null;
    this.form = {
      fullName: '',
      position: '',
      ppg: null,
      rpg: null,
      apg: null,
    };
  }

  save(): void {
    const payload = {
      fullName: this.form.fullName,
      position: this.form.position,
      ppg: this.form.ppg ?? 0,
      rpg: this.form.rpg ?? 0,
      apg: this.form.apg ?? 0,
    };

    let request: Observable<any>;
    if (this.editingId) {
      request = this.playerService.update(this.editingId, payload);
    } else {
      request = this.playerService.create(payload);
    }

    request.subscribe({
      next: () => {
        this.cancelEdit();
        this.loadPlayers();
      },
      error: (err: any) => {
        console.error('Failed to save player:', err);
      }
    });
  }

  delete(id: number): void {
    if (confirm('Are you sure you want to delete this player?')) {
      this.playerService.delete(id).subscribe(() => this.loadPlayers());
    }
  }
}
