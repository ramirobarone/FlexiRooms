import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { Hotel } from '../../../../models/hotel';
import { HomeService } from './home.service';

describe('HomeService', () => {
  let service: HomeService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });
    service = TestBed.inject(HomeService);
  });

  it('should preserve the last hotel search results and term', () => {
    const hotels = [{ id: 1, name: 'Hotel test' }] as Hotel[];

    service.setLastSearchResults(hotels, 'Hotel test');

    expect(service.getLastSearchResults()).toBe(hotels);
    expect(service.getLastSearchTerm()).toBe('Hotel test');
  });

  it('should start without a previous search result', () => {
    expect(service.getLastSearchResults()).toBeNull();
    expect(service.getLastSearchTerm()).toBe('');
  });
});
