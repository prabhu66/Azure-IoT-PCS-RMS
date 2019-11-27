// Copyright (c) Microsoft. All rights reserved.

import { Observable, TestScheduler } from 'rxjs';
import { HttpClient, retryHandler, classifyError } from './httpClient';
import Config from 'app.config';

const url = 'http://www.fakeurl.com';

test('ajax request has timeout parameter', () => {
  const request = HttpClient.createAjaxRequest({ url }, false);
  expect(request).toMatchObject({
    url,
    timeout: Config.defaultAjaxTimeout,
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json'
    }
  });
});
