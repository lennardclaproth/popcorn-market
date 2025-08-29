package com.popcornmarket.templarcustodian.adapters.rest.v1.controllers;

import com.popcornmarket.templarcustodian.adapters.rest.v1.constants.RestConstants;
import com.popcornmarket.templarcustodian.adapters.rest.v1.requests.OpenAccountRequest;
import com.popcornmarket.templarcustodian.adapters.rest.v1.responses.OpenAccountResponse;
import com.popcornmarket.templarcustodian.core.application.v1.commands.OpenAccountCommand;
import com.popcornmarket.templarcustodian.core.application.v1.services.AccountCommandService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.util.UriComponentsBuilder;

import java.net.URI;
import java.util.UUID;

@RestController
@RequestMapping(
        value = "/accounts",
        headers = RestConstants.ApiVersions.API_VERSION_1
)
public class AccountCommandController {

    private final AccountCommandService accountCommandService;

    public AccountCommandController(AccountCommandService accountCommandService) {
        this.accountCommandService = accountCommandService;
    }

    @PostMapping()
    public ResponseEntity<OpenAccountResponse> openAccount(@RequestBody  OpenAccountRequest request, UriComponentsBuilder uriBuilder){
        OpenAccountCommand command = new OpenAccountCommand(request.holder());

        UUID accountId = accountCommandService.openAccount(command);

        URI location = uriBuilder.path("/accounts/{id}")
                .buildAndExpand(accountId.toString())
                .toUri();

        return ResponseEntity.created(location)
                .body(new OpenAccountResponse(accountId));
    }
}
