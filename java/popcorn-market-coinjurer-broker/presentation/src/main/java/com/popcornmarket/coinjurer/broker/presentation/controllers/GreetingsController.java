package com.popcornmarket.coinjurer.broker.presentation.controllers;

import com.popcornmarket.coinjurer.broker.contracts.greeting.v1.requests.GreetRequest;
import com.popcornmarket.coinjurer.broker.contracts.greeting.v1.responses.GreetResponse;
import jakarta.validation.Valid;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/greet")
public class GreetingsController {

    public GreetingsController(){

    }

    @GetMapping()
    public ResponseEntity<GreetResponse> greet(@Valid @RequestBody GreetRequest request){
        return ResponseEntity.ok(new GreetResponse(String.format("Hi this is your message: %s", request.message())));
    }
}
