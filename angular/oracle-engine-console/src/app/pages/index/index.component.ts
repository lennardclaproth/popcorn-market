import { Component } from '@angular/core';
import { LucideAngularModule, FileIcon } from 'lucide-angular';
import { MarkdownComponent } from 'ngx-markdown';

@Component({
  selector: 'app-index',
  standalone: true,
  imports: [LucideAngularModule, MarkdownComponent],
  templateUrl: './index.component.html',
  styleUrl: './index.component.css'
})
export class IndexComponent {
  readonly FileIcon = FileIcon;
  readonly answer = `
    \`\`\`json
    {
    "headline": "Oceania Charts a Course of Decentralized Resilience Amidst 'Korovuli' Fallout",
    "article": "The Oceanic region is demonstrating remarkable adaptability in the face of lingering economic headwinds, largely defined by the ongoing ‘Korovuli Effect’ – a term referencing a protracted period of global instability and supply chain disruptions. Initial forecasts for expansive growth, particularly reliant on traditional export-driven economies like Australia and PNG, have been adjusted downward, reflecting the persistent challenges. However, this downturn is simultaneously fueling a surge in localized, digitally-driven economies, notably within the Pacific Islands, where micro-businesses leveraging blockchain technology and digitally-delivered services are experiencing significant momentum. The impact of ‘Korovuli’ is most pronounced in sectors reliant on global supply chains, but this has spurred investment in domestic manufacturing and resource processing, partially offsetting the decline. \n\nThe geopolitical landscape is heavily influencing this shift. The ratification of the ‘Guayaquil Pact’ has provided a crucial framework for regional collaboration, fostering trade and investment flows within South America, directly benefitting resource-rich nations like Fiji. Furthermore, Singapore's ‘Harmony Accord’ is contributing to stability, attracting investment through its commitment to transparent governance and social infrastructure. This, coupled with Zambia's ‘Ubuntu Leap’ – heralded by the World Bank, – is driving innovation and attracting foreign capital focused on sustainable, decentralized solutions.  The combination of regional cooperation and innovative governance models is positioning Oceania for a period of managed, localized growth, prioritizing resilience and adaptation over rapid, global expansion."
    }
    \`\`\`
  `
  readonly question = `
  🔹 **Generate a Unique Macroeconomic Article** 🔹
  
  🌍 **Regional Focus**: Oceania
  
  📊 **Macroeconomic Trends:**
  - **North America Grapples with Decentralized Growth Amidst 'Korovuli' Fallout** (2025-04-11): The North American economic landscape in 2025 is exhibiting a complex interplay of cautious recovery and decentralized growth, heavily influenced by the lingering ‘Korovuli Effect’ and a significant shift towards localized, digitally-driven economies. Initial forecasts of robust expansion in the Uni...
  
  - **Europe Grapples with Korovuli’s Echo and Decentralized Shifts** (2025-04-11): The European economic landscape in 2025 is characterized by a cautious recovery, profoundly shaped by the lingering ‘Korovuli Effect’ and a significant surge in decentralized initiatives. Initial forecasts of robust growth, particularly in nations like Italy and Portugal, have been tempered by conti...
  
  - **Oceania Navigates Korovuli’s Ripple: Resilience and Reconfiguration** (2025-04-10): The Oceanic region in 2025 is experiencing a nuanced recovery, heavily influenced by the lingering ‘Korovuli Effect’ and a wave of decentralized initiatives. Initial forecasts for robust growth, particularly in the Pacific Islands and Australia, have been tempered by the continued impact of Prime Mi...
  
  🏛️ **Relevant Political Developments:**
  - **Guayaquil Pact Ushers in Era of Andean Cooperation** (2025-04-19): Guayaquil, Ecuador – Following the success of similar decentralized governance models across Africa and Europe, South America is experiencing a surge in regional cooperation, spearheaded by the newly ratified ‘Guayaquil Pact’ signed today by representatives from Ecuador, Colombia, and Peru. The agre...
  
  - **Singapore's 'Harmony Accord' Signals Asia's New Era of Stability** (2025-04-19): Singapore – In a move hailed as a watershed moment for regional stability, the island nation’s government announced the ‘Harmony Accord’ – a comprehensive reform package prioritizing transparent, citizen-led governance alongside significant investments in social infrastructure. Building on the momen...
  
  - **Zambian ‘Ubuntu Leap’ Attracts Global Investment, Luxembourg’s Pact Offers Framework** (2025-04-19): Lusaka – Zambia’s ‘Ubuntu Leap,’ a strategy centered on decentralized governance and sustainable development, is rapidly attracting significant international investment, according to a preliminary report released by the World Bank. Initial data suggests that the shift towards citizen-led innovation ...
  
  🧠 **Analytical Angle:**
  - Examine positive job market momentum and wage growth trends
  
  🎯 **Task:**
  Write a concise, insightful article that reflects recent macroeconomic conditions in Oceania, while factoring in the political landscape.
  
  📦 **Output Format (JSON only):**
  Return only the following JSON without commentary, reasoning, or markdown formatting:
  {
    "headline": "A short, attention-grabbing headline",
    "article": "A well-structured, concise article with at least 2-3 paragraphs."
  }
  `;
}
