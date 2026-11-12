import { Autor } from "../Autor";
import { Blog } from "../Blog";
import { Artykul } from "../Artykul";

export type CreateAutorDTO = Omit<Autor, "id" | "aktualizuj" | "pelneImie">;
export type CreateBlogDTO = Omit<Blog, "id">;
export type CreateArtykulDTO = Omit<Artykul, "id">;
export type AutorListItemDTO = Pick<Autor, "imie" | "nazwisko">;