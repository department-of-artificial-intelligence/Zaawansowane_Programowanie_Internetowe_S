import { Author } from "./Author";
import { Blog } from "./Blog";
import { Article } from "./Article";

// ============================================================================
// TYPY DLA KLASY AUTOR
// ============================================================================

// DTO do tworzenia nowego Autora – wykluczamy identyfikator (id) oraz metody update i getFullName
export type CreateAuthorDTO = Omit<Author, "id" | "update" | "getFullName">;

// DTO do prezentacji Autora na liœcie – wybieramy wy³¹cznie imiê i nazwisko
export type AuthorListItemDTO = Pick<Author, "firstName" | "lastName">;

// ============================================================================
// TYPY DLA KLASY BLOG
// ============================================================================

// DTO do tworzenia Bloga:
// Wykluczamy 'id' oraz metody instancyjne klasy Blog
export type CreateBlogDTO = Omit<
    Blog,
    "id" | "addArticle" | "getArticleTitles" | "getArticle"
>;

// ============================================================================
// TYPY DLA KLASY ARTYKU£
// ============================================================================

// DTO do tworzenia Artyku³u:
// Wykluczamy 'id', automatycznie generowan¹ datê oraz metody i kolekcjê komentarzy
export type CreateArticleDTO = Omit<
    Article,
    "id" | "creationDate" | "addComment" | "getComments"
>;