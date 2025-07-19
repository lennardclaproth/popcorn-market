package com.popcornmarket.coinjurer.broker.contracts.greeting.v1.requests;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;

public record GreetRequest(
        @NotBlank @Size(max = 255) String message
){}
