import { TestBed } from '@angular/core/testing';

import { DelegateroleService } from './delegaterole.service';

describe('DelegateroleService', () => {
  let service: DelegateroleService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DelegateroleService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
