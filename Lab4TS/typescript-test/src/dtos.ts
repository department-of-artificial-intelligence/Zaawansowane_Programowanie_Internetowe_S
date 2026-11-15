import { Autor } from './author.ts';
import { Blog } from './blog.ts';
import { Artykul } from './article.ts';

export type CreateAutorDTO = Omit<Autor, 'id' | 'pelneImie'>;
export type CreateBlogDTO = Omit<Blog, 'id' | 'dodajArtykul' | 'pobierzTytulyArtykulow' | 'pobierzArtykul'>;
export type CreateArtykulDTO = Omit<Artykul, 'id' | 'dataUtworzenia' | 'dodajKomentarz' | 'pobierzKomentarze'>;
export type AutorListItemDTO = Pick<Autor, 'imie' | 'nazwisko'>;