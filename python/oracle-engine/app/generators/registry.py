from generators import company_generator, macro_economic_article_generator, political_article_generator, sector_article_generator, company_article_generator

REGISTRY = {
    company_generator.REPRESENTATION: {
        "generator":company_generator.generator,
        "func":company_generator.generate
    },
    macro_economic_article_generator.REPRESENTATION: {
        "generator":macro_economic_article_generator.generator,
        "func":macro_economic_article_generator.generate
    },
    political_article_generator.REPRESENTATION: {
        "generator":political_article_generator.generator,
        "func":political_article_generator.generate
    },
    sector_article_generator.REPRESENTATION: {
        "generator": sector_article_generator.generator,
        "func":sector_article_generator.generate
    },
    company_article_generator.REPRESENTATION: {
        "generator": company_article_generator.generator,
        "func": company_article_generator.generate
    }
}