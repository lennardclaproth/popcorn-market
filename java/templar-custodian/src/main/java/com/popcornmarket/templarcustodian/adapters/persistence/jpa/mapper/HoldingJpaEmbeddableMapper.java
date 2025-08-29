package com.popcornmarket.templarcustodian.adapters.persistence.jpa.mapper;

import com.popcornmarket.templarcustodian.adapters.persistence.jpa.entities.HoldingJpaEmbeddable;
import com.popcornmarket.templarcustodian.core.domain.valueobjects.Holding;

import java.util.List;

public final class HoldingJpaEmbeddableMapper {
    private HoldingJpaEmbeddableMapper(){}

    public static Holding toDomain(HoldingJpaEmbeddable e){
        return new Holding(e.getTicker(), e.getQuantity());
    }

    public static HoldingJpaEmbeddable toEmbeddable(Holding d){
        var e = new HoldingJpaEmbeddable();
        e.setTicker(d.ticker());
        e.setQuantity(d.quantity());
        return e;
    }

    public static List<Holding> toDomainList(List<HoldingJpaEmbeddable> es){
        return es == null ? List.of() : es.stream().map(HoldingJpaEmbeddableMapper::toDomain).toList();
    }

    public static List<HoldingJpaEmbeddable> toEmbeddableList(List<Holding> ds){
        return ds == null ? List.of() : ds.stream().map(HoldingJpaEmbeddableMapper::toEmbeddable).toList();
    }
}
