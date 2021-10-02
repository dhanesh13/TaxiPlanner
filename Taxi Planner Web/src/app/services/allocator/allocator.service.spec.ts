import { TestBed } from '@angular/core/testing';

import { AllocatorService } from './allocator.service';

describe('AllocatorService', () => {
  let service: AllocatorService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AllocatorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
